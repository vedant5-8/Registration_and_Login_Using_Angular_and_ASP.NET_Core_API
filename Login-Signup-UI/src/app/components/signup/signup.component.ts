import { Component, OnInit } from '@angular/core';
import { faUser, faLock, faEye, faEyeSlash, faEnvelope } from '@fortawesome/free-solid-svg-icons';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import ValidateForm from 'src/app/helpers/validateForm';
import { AuthService } from 'src/app/services/auth.service';
import { Router } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.css']
})
export class SignupComponent implements OnInit {

  faUser = faUser;
  faLock = faLock;
  faEye = faEye;
  faEyeSlash = faEyeSlash;
  faEnvelope = faEnvelope;
  
  showPassword: boolean = false;

  signupForm!: FormGroup;

  constructor(
    private fb: FormBuilder, 
    private auth: AuthService,
    private router: Router,
    private toast: NgToastService
  ) {

  }

  ngOnInit() {
    this.signupForm = this.fb.group({
      firstname: ['', Validators.required],
      lastname: ['', Validators.required],
      email: ['', Validators.required],
      username: ['', Validators.required],
      password: ['', Validators.required],
    })
  }

  showHidePass() {
    this.showPassword = !this.showPassword;
  }

  onSignUp() {
    if (this.signupForm.valid) {

      // Send the object to database
      this.auth.signUp(this.signupForm.value)
      .subscribe({
        next: (res => {
          this.toast.success({ detail: "SUCCESS", summary: res.message, duration: 1500 });
          this.signupForm.reset();
          this.router.navigate(['login']);
        }),
        error: (err => {
          this.toast.error({ detail: "ERROR", summary: err?.error.message, sticky: true })
        })
      })

    } 
    
    else {

      ValidateForm.validateAllFormsField(this.signupForm);
      this.toast.warning({ detail: "WARN", summary: 'Your form is invalid.', sticky: true });
      
    }
  }

}
