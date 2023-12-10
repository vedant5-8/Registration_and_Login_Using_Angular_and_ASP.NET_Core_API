import { Component, OnInit } from '@angular/core';
import { NgToastService } from 'ng-angular-popup';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  constructor(private auth : AuthService, private toast : NgToastService) {}
  
  ngOnInit(): void {
    throw new Error('Method not implemented.');
  }

  logout() {
    this.toast.warning({detail: "WARNING", summary: "Logout Successfully", duration: 1000 });
    this.auth.logout();
  }

}
