import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { inject } from "@angular/core/primitives/di";
import { Observable } from "rxjs";

export interface RegisterRequest {
  email: string;
  password: string;
  displayName: string;
}

@Injectable({ providedIn: 'root' })

export class RegistrationService {
  private http = inject(HttpClient);
  private apiURL = 'https://localhost:7275/JWTAuth/register';

  register(request: RegisterRequest): Observable<any> {
    return this.http.post<any>(this.apiURL, request);
  }
}
