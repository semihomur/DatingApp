import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {BehaviorSubject, Observable} from 'rxjs';
import {map} from 'rxjs/operators';
import {JwtHelperService} from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
@Injectable({
  providedIn: 'root'
})
export class AuthService {
baseUrl = environment.apiUrl + 'token/';
jwtHelper = new JwtHelperService();
decodedToken: any;
user: User;
photoUrl = new BehaviorSubject<string>('../../assets/user.png');
currentPhotoUrl = this.photoUrl.asObservable();
constructor(private http: HttpClient) { }
changeMemberPhoto(photoUrl: string ) {
  this.photoUrl.next(photoUrl);
}
login(model: any) {
  const grantType = 'password';
  return this.http.post(this.baseUrl + 'login', {username: model.username, password: model.password, grantType})
    .pipe( map((response: any) => {
      if (response) {
        localStorage.setItem('token', response .authToken.token);
        localStorage.setItem('user', JSON.stringify(response.user));
        localStorage.setItem('refreshToken', response.authToken.refresh_token);
        localStorage.setItem('username', response.authToken.username);
        this.decodedToken = this.jwtHelper.decodeToken(response.token);
        this.user = response.user;
        this.changeMemberPhoto(this.user.photoUrl);
      }
    }));
}
getNewRefreshToken(): Observable<any> {
    const username = localStorage.getItem('username');
    const refreshToken = localStorage.getItem('refreshToken');
    const grantType = 'refresh_token';
    return this.http.post<any>(this.baseUrl, {username, refreshToken,  grantType}).pipe(
        map(result => {
            if (result && result.authToken.token) {
                // this.loginStatus.next(true);
                localStorage.setItem('token', result.authToken.token);
                localStorage.setItem('username', result.authToken.username);
                localStorage.setItem('refreshToken', result.authToken.refresh_token);
            }
            return result as any;
        })
        );
}

register(model: User) {
  return this.http.post(this.baseUrl + 'register', model);
}
loggedIn() {
  try {
    const token = localStorage.getItem('token');
    return !this.jwtHelper.isTokenExpired(token);
  } catch {
    return false;
  }
}
logout() {
  localStorage.removeItem('token');
  localStorage.removeItem('user');
  this.decodedToken = null;
  this.user = null;
}
  roleMatch(allowedRoles): boolean {
    let isMatch = false;
    const userRoles = this.decodedToken.role as Array<string>;
    allowedRoles.forEach(element => {
      if (userRoles.includes(element)) {
        isMatch = true;
        return;
      }
    });
    return isMatch;
  }
}
