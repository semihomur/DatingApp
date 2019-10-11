import { Component, OnInit } from '@angular/core';
import {JwtHelperService} from '@auth0/angular-jwt';
import { AuthService } from './_services/auth.service';
import { User } from './_models/user';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'DatingApp-SPA';
  jwtHelper = new JwtHelperService();
  constructor(private authService: AuthService) {}
  ngOnInit() {
    const token = localStorage.getItem('token');
    const user: User = JSON.parse(localStorage.getItem('user'));
    if (token) {
      this.authService.decodedToken = this.jwtHelper.decodeToken(token);
  }
    if (user) {
      this.authService.user = user;
      this.authService.changeMemberPhoto(user.photoUrl);
    }
  }
}
