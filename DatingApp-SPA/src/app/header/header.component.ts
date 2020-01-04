import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {
  model: any = {};
  photoUrl: string;
  @ViewChild('loginForm', {static: false}) loginForm: NgForm;
  constructor(public authService: AuthService, private alertify: AlertifyService, private router: Router) { }

  ngOnInit() {
    this.authService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl);
  }
  login() {
    try {
      this.authService.login(this.model).subscribe(next => {
        this.alertify.success('Logged in successfully');
      }, error => {
        this.alertify.error('Password or username is wrong');
      }, () => {
        this.router.navigate(['/members']);
      }
      );
    } catch {
      this.alertify.error('Error');
    }
  }
  loggedIn() {
    return this.authService.loggedIn();
  }
  logout() {
    this.alertify.message('Logged out');
    this.router.navigate(['/']);
    this.authService.logout();
  }
}
