import {Component, OnInit} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { catchError, finalize, switchMap } from 'rxjs/operators';
import { UserDetailDto, UserService } from '../../../services/user.service';

@Component({
  standalone: true,
  selector: 'app-users-list',
  imports: [CommonModule, FormsModule, RouterLink],
    templateUrl: './users-list.component.html',
})
export class UsersListComponent implements OnInit {
  query = '';
  loading = false;
  error: string | null = null;

  users: UserDetailDto[] = [];

  constructor(private readonly userService: UserService) {}

  ngOnInit(): void {
    this.refresh();
  }

  refresh(): void {
    this.loading = true;
    this.error = null;

    // Per requirement: create users from Data.cs first (idempotent), then load user01/user02.
    this.userService
        .createUsersFromSeed()
        .pipe(
            switchMap(() =>
                forkJoin([
                  this.userService.getUserById('user01').pipe(catchError(() => of(null))),
                  this.userService.getUserById('user02').pipe(catchError(() => of(null)))
                ])
            ),
            finalize(() => (this.loading = false))
        )
        .subscribe({
          next: (result) => {
            this.users = result.filter((x): x is UserDetailDto => !!x);
          },
          error: (err) => {
            this.error = this.toErrorMessage(err);
          }
        });
  }

  filteredUsers(): UserDetailDto[] {
    const q = this.query.trim().toLowerCase();
    if (!q) return this.users;

    return this.users.filter((u) => {
      const fullName = `${u.firstName ?? ''} ${u.lastName ?? ''}`.trim().toLowerCase();
      return (
          (u.id ?? '').toLowerCase().includes(q) ||
          (u.userId ?? '').toLowerCase().includes(q) ||
          (u.username ?? '').toLowerCase().includes(q) ||
          fullName.includes(q)
      );
    });
  }

  displayName(u: UserDetailDto): string {
    const fn = (u.firstName ?? '').trim();
    const ln = (u.lastName ?? '').trim();
    return `${fn} ${ln}`.trim();
  }

  displayAge(u: UserDetailDto): string {
    return u.age === null || u.age === undefined ? '-' : String(u.age);
  }

  rolesCount(u: UserDetailDto): number {
    return u.roles?.length ?? 0;
  }

  permissionsCount(u: UserDetailDto): number {
    return u.permissions?.length ?? 0;
  }

  private toErrorMessage(err: any): string {
    if (!err) return 'Unknown error.';
    if (typeof err === 'string') return err;
    if (err?.error?.title) return err.error.title;
    if (err?.message) return err.message;
    return 'Request failed.';
  }
}
