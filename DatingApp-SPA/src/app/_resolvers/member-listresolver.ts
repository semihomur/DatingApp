import { Injectable } from '@angular/core';
import { User } from '../_models/user';
import {Resolve, Router} from '@angular/router';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class MemberListResolver implements Resolve<User[]> {
    pageNumber = 1;
    pageSize = 5;
    constructor(private userService: UserService, private router: Router, private aletfify: AlertifyService) { }
    resolve(): Observable<User[]> {
        return this.userService.getUsers(this.pageNumber, this.pageSize).pipe(
            catchError(error => {
                this.aletfify.error('Problems retrieving data');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}