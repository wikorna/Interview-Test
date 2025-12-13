import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { UserDetailDto, UserService } from '../../../services/user.service';

@Component({
    standalone: true,
    selector: 'app-user-detail',
    imports: [CommonModule, RouterLink],
    templateUrl: './user-detail.component.html',
})
export class UserDetailComponent implements OnInit {
    loading = false;
    error: string | null = null;
    user: UserDetailDto | null = null;

    constructor(private readonly route: ActivatedRoute, private readonly userService: UserService) {}

    ngOnInit(): void {
        const id = this.route.snapshot.paramMap.get('id') ?? '';
        if (!id) {
            this.error = 'userId is required.';
            return;
        }

        this.loading = true;
        this.error = null;

        this.userService
            .getUserById(id)
            .pipe(finalize(() => (this.loading = false)))
            .subscribe({
                next: (u) => (this.user = u),
                error: (err) => (this.error = this.toErrorMessage(err))
            });
    }

    fullName(): string {
        if (!this.user) return '';
        return `${this.user.firstName ?? ''} ${this.user.lastName ?? ''}`.trim();
    }

    displayAge(): string {
        if (!this.user) return '';
        return this.user.age === null || this.user.age === undefined ? 'N/A' : String(this.user.age);
    }

    private toErrorMessage(err: any): string {
        if (!err) return 'Unknown error.';
        if (typeof err === 'string') return err;
        if (err?.error?.title) return err.error.title;
        if (err?.message) return err.message;
        return 'Request failed.';
    }
}
