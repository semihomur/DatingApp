import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../../_services/auth.service';
import { AlertifyService } from '../../_services/alertify.service';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { User } from '../../_models/user';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  user: User;
  @Output() cancelRegister = new EventEmitter();
  constructor(private authService: AuthService, private alertify: AlertifyService, private formBuilder: FormBuilder,
              private router: Router) { }
  registerForm: FormGroup;
  bsConfig: Partial<BsDatepickerConfig>;
  ngOnInit() {
    this.createFormBuilder();
    this.bsConfig = {
      containerClass: 'theme-red'
    };
  }
  createFormBuilder() {
    this.registerForm = this.formBuilder.group({
      gender: ['male'],
      username: [null, Validators.required],
      email: [null, [Validators.required, Validators.minLength(6), Validators.maxLength(30), Validators.pattern(/^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/)]],
      emailCode: [null, [Validators.required, Validators.maxLength(6), Validators.minLength(6)]],
      knownAs: [null, Validators.required],
      dateOfBirth: [null, Validators.required],
      city: [null, Validators.required],
      country: [null, Validators.required],
      password : [null, [Validators.required, Validators.minLength(6), Validators.maxLength(12)]],
      confirmPassword : [null, [Validators.required, Validators.minLength(6), Validators.maxLength(12)]]
    }, {validator : this.passwordMatchValidator});
  }
  passwordMatchValidator(g: FormGroup) {
    // tslint:disable-next-line:object-literal-key-quotes
    return g.get('password').value === g.get('confirmPassword').value ? null : { 'mismatch': true};
  }
  register() {
    this.user = Object.assign({}, this.registerForm.value);
    this.authService.register(this.user).subscribe(response => {
      this.alertify.success('Registration successful');
    }, error => {
      // this.alertify.error(error.error);
    }, () => {
      this.authService.login(this.user).subscribe(() => {
        this.router.navigate(['/members']);
      }
      );
    }
    );
  }
  cancel() {
    this.cancelRegister.emit(false);
  }
  SendCode() {
    this.authService.SendCode(this.registerForm.controls.email.value).subscribe(() => {
      this.alertify.success('Email sent');
    });
  }

}
