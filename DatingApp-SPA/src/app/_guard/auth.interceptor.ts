import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../_services/auth.service';
import { Observable, BehaviorSubject, throwError } from 'rxjs';
import { catchError, switchMap, finalize, filter, take } from 'rxjs/operators';
import { AlertifyService } from '../_services/alertify.service';

@Injectable({
    providedIn: 'root'
})

export class AuthInterceptor implements HttpInterceptor {
    private isTokenRefreshing: boolean;
    tokenSubject: BehaviorSubject<string> = new BehaviorSubject<string>(null);
    constructor(private acct: AuthService, private alertify: AlertifyService) {}
    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(this.attachTokenToRequest(request))
            .pipe(
                catchError((err): Observable<any> => {
                    if (err instanceof HttpErrorResponse) {
                        switch ((err as HttpErrorResponse).status) {
                            case 401:
                                console.log('Token expired. Attemtping refresh');
                                return this.HandleHttpResponseError(request, next);
                            case 400:
                                return Observable.throw(err);
                        }
                    } else {
                        return throwError(this.HandleError);
                    }
                })
            )
        ;
    }
    private HandleError(errorResponse: HttpErrorResponse) {
        let errorMsg: string;
        if (errorResponse.error instanceof Error) {
            errorMsg = 'An error occured:' + errorResponse.error.message; // clientside network
        } else {
            errorMsg = `Backend returned code ${errorResponse.status}, body was: $(errorResponse.error)`;
        }
        return throwError(errorMsg);
    }
    private HandleHttpResponseError(request: HttpRequest<any>, next: HttpHandler) {
        if (!this.isTokenRefreshing) {
            this.isTokenRefreshing = true;
            this.tokenSubject.next(null);
            return this.acct.getNewRefreshToken().pipe(
                switchMap((tokenResponse: any) => {
                    if (tokenResponse) {
                        localStorage.setItem('token', tokenResponse.authToken.token);
                        localStorage.setItem('username', tokenResponse.authToken.username);
                        localStorage.setItem('user', JSON.stringify(tokenResponse.user));
                        localStorage.setItem('refreshToken', tokenResponse.authToken.refresh_token);
                        console.log('Token refreshed');
                        return next.handle(this.attachTokenToRequest(request));
                    }
                    return this.acct.logout() as any;
                }),
                catchError(err => {
                    this.acct.logout();
                    return this.HandleError(err);
                }),
                finalize(() => {
                    this.isTokenRefreshing = false;
                })
            );
        } else {
            this.isTokenRefreshing = false;
            return this.tokenSubject.pipe(filter(token => token != null),
            take(1),
            switchMap((token => {
                return next.handle(this.attachTokenToRequest(request));
            }))
            );
        }

    }
    private attachTokenToRequest(request: HttpRequest<any>) {
        const token = localStorage.getItem('token');
        return request.clone({
            setHeaders: {
                Authorization: `Bearer ${token}`
            }
        });
    }
}
/*
Access Token
        const currentUser = this.acct.loggedIn();
        const token = localStorage.getItem('token');
        if (currentUser && token !== undefined) {
            request = request.clone({
                setHeaders: {
                    Authorization: `Bearer ${token}`
                }
            });
        }
        return next.handle(request);
*/
