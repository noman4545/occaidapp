import { NewPasswordComponent } from './components/new-password/new-password.component';
import { ManageUsersComponent } from './components/manage-users/manage-users.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AdminGuard } from '../auth/authGuard/admin.guard';
import { ErrorComponent } from '../main/components/error/error.component';
import { ACIDAdminComponent } from './components/acidadmin/acidadmin.component';
import { AdminDashboardComponent } from './components/admin-dashboard/admin-dashboard.component';
import { IOSCodesAdminComponent } from './components/ioscodes-admin/ioscodes-admin.component';
import { LogsComponent } from './components/logs/logs.component';
import { RegisterComponent } from './components/register/register.component';
import { SMSAdminComponent } from './components/sms-admin/sms-admin.component';
import { TMCSAdminComponent } from './components/tmcs-admin/tmcs-admin.component';
import { TMCSAdminComponentLatest } from './components/tmcs-admin-latest/tmcs-admin-latest.component';

const routes: Routes = [
  {
    path: '', component: AdminDashboardComponent, children: [
      { path: '', pathMatch: 'full', redirectTo: 'ioscodes' },
      { path: 'ioscodes', component: IOSCodesAdminComponent, canActivate: [AdminGuard] },
      { path: 'acid', component: ACIDAdminComponent, canActivate: [AdminGuard] },
      { path: 'tmcs', component: TMCSAdminComponent, canActivate: [AdminGuard] },
      { path: 'tmcs-latest', component: TMCSAdminComponentLatest, canActivate: [AdminGuard] },
      { path: 'sms', component: SMSAdminComponent, canActivate: [AdminGuard] },
      { path: 'logs', component: LogsComponent, canActivate: [AdminGuard] },
      { path: 'register-users', component: RegisterComponent, canActivate: [AdminGuard] },
      { path: 'manage-users', component: ManageUsersComponent, canActivate: [AdminGuard] },
      { path: 'change-password/:id', component: NewPasswordComponent, canActivate: [AdminGuard] },
      { path: '**', component: ErrorComponent },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
