import { Component, OnInit } from '@angular/core';
import { faUser, faLock, faEye, faEyeSlash } from '@fortawesome/free-solid-svg-icons';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import ValidateForm from 'src/app/helpers/validateForm';
import { AuthService } from 'src/app/services/auth.service';
import { Router } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  
  faUser = faUser;
  faLock = faLock;
  faEye = faEye;
  faEyeSlash = faEyeSlash;
  
  showPassword: boolean = false;

  loginForm!: FormGroup;

  constructor (
    private fb: FormBuilder, 
    private auth: AuthService,
    private router: Router,
    private toast: NgToastService
  ) {}

  ngOnInit() {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    })
  }

  showHidePass() {
    this.showPassword = !this.showPassword;
  }

  onLogin() {
    if(this.loginForm.valid) {

      // Send the object to database
      this.auth.login(this.loginForm.value)
      .subscribe({
        next:(res) => {
          this.loginForm.reset();
          this.auth.storeToken(res.token);
          this.toast.success({detail: "SUCCESS", summary: res.message, duration: 2000});
          this.router.navigate(['dashboard']);
        },
        error: (err) => {
          this.toast.error({ detail: "ERROR", summary: err?.error.message, sticky: true });
        }
      })

    } else {
      // throw the error using toaster and with required fields
      ValidateForm.validateAllFormsField(this.loginForm);
      this.toast.warning({ detail: "WARN", summary: 'Your form is Invalid.', sticky: true })
    }
  }

}
