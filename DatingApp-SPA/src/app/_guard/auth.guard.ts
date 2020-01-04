import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { catchError, tap } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  jwtHelper = new JwtHelperService();
  constructor(private authService: AuthService, private router: Router, private alertify: AlertifyService) {}

  canActivate(next: ActivatedRouteSnapshot): boolean {
    // tslint:disable-next-line:no-string-literal
    const roles = next.firstChild.data['roles'] as Array<string>;
    if (roles) {
      const match = this.authService.roleMatch(roles);
      if (match) {
        return true;
      } else {
        this.router.navigate(['members']);
        this.alertify.error('You are not authorized to access this area');
      }
    }
    this.authService.getNewRefreshToken().pipe(
      tap((tokenResponse: any) => {
        if (tokenResponse) {
             localStorage.setItem('user', JSON.stringify(tokenResponse.user));
             localStorage.setItem('refreshToken', tokenResponse.authToken.refresh_token);
             localStorage.setItem('username', tokenResponse.authToken.username);
             console.log('Token refreshed');
             this.authService.decodedToken = this.jwtHelper.decodeToken(tokenResponse.authToken.token);
             this.authService.user = tokenResponse.user;
             this.authService.refreshedRole.next(1);
         } else {
           this.router.navigate(['/']);
           return this.authService.logout() as any;
         }
     }),
     catchError(err => {
         // this.authService.logout();
         return throwError('Role error');
     })).subscribe();
    if (this.authService.loggedIn()) {
    return true;
  }
    return false;
  }
}
