import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface UserRoleDto {
  roleId: number;
  roleName: string;
}

export interface UserDetailDto {
  id: string;
  userId: string;
  username: string;
  firstName: string;
  lastName: string;
  age: number | null;
  roles: UserRoleDto[];
  permissions: string[];
}

@Injectable({ providedIn: 'root' })
export class UserService {
  private readonly apiBaseUrl = 'https://localhost:44375/gateway/api/user';
  private readonly apiKeyPlainText = 'DEV-INTERVIEW-KEY-2025';

  constructor(private readonly http: HttpClient) {}

  private headers(): HttpHeaders {
    return new HttpHeaders({
      'x-api-key': this.apiKeyPlainText,
      'Content-Type': 'application/json'
    });
  }

  createUsersFromSeed(): Observable<{ affected: number } | any> {
    return this.http.post(`${this.apiBaseUrl}/CreateUser`, {}, { headers: this.headers() });
  }

  getUserById(userId: string): Observable<UserDetailDto> {
    return this.http.get<UserDetailDto>(`${this.apiBaseUrl}/GetUserById/${encodeURIComponent(userId)}`,
        { headers: this.headers() }
    );
  }
}
