import { NewPasswordComponent } from './components/new-password/new-password.component';
import { ManageUsersComponent } from './components/manage-users/manage-users.component';
import { NgModule } from '@angular/core';
import { AdminRoutingModule } from './admin-routing.module';
import { AdminDashboardComponent } from './components/admin-dashboard/admin-dashboard.component';
import { IOSCodesAdminComponent } from './components/ioscodes-admin/ioscodes-admin.component';
import { ACIDAdminComponent } from './components/acidadmin/acidadmin.component';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TMCSAdminComponent } from './components/tmcs-admin/tmcs-admin.component';
import { LogsComponent } from './components/logs/logs.component';
import { SMSAdminComponent } from './components/sms-admin/sms-admin.component';
import { RegisterComponent } from './components/register/register.component';

@NgModule({
  declarations: [
    AdminDashboardComponent,
    IOSCodesAdminComponent,
    ACIDAdminComponent,
    TMCSAdminComponent,
    LogsComponent,
    SMSAdminComponent,
    RegisterComponent,
    ManageUsersComponent,
    NewPasswordComponent
  ],
  imports: [
    CommonModule,
    AdminRoutingModule,
    ReactiveFormsModule,
    FormsModule
  ]
})
export class AdminModule { }
