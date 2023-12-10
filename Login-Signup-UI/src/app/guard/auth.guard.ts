import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { NgToastService } from 'ng-angular-popup';

export const authGuard: CanActivateFn = (route, state) => {

  const auth = inject(AuthService);
  const router = inject(Router);
  const toast = inject(NgToastService);

  if (auth.isLoggedIn()) {
    return true;
  } else {
    toast.error({detail: "ERROR", summary: "Please Login First", duration: 2000});
    router.navigate(['login']);
    return false;
  }
};

