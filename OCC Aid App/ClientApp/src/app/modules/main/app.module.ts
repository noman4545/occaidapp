import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { ToastrModule } from 'ngx-toastr';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ErrorComponent } from './components/error/error.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { IOSCodesComponent } from './components/ioscodes/ioscodes.component';
import { ACIDComponent } from './components/acid/acid.component';
import { IOSCodesService } from 'src/app/services/ioscodes.service';
import { ACIDService } from 'src/app/services/acid.service';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NotificationService } from 'src/app/services/notification.service';
import { MainComponent } from './components/main/main.component';
import { CentralComponent } from './components/central/central.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { LoggingService } from 'src/app/services/logging.service';
import { TMCSComponent } from './components/tmcs/tmcs.component';
import { NotifyTMCSService } from 'src/app/services/notifyTMCS.service';
import { SMSService } from 'src/app/services/sms.service';
import { SMSComponent } from './components/sms/sms.component';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { LoginGuard } from '../auth/authGuard/login.guard';
import { AuthGuard } from '../auth/authGuard/auth.guard';
import { AdminGuard } from '../auth/authGuard/admin.guard';
import { RequestInterceptor } from 'src/app/interceptors/request.interceptor';

@NgModule({
  declarations: [
    AppComponent,
    ErrorComponent,
    IOSCodesComponent,
    ACIDComponent,
    MainComponent,
    CentralComponent,
    TMCSComponent,
    SMSComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    ReactiveFormsModule,
    FormsModule,
    HttpClientModule,
    ToastrModule.forRoot()
  ],
  providers: [IOSCodesService, ACIDService, NotificationService, 
              LoggingService, NotifyTMCSService, SMSService, 
              AuthenticationService, LoginGuard, AuthGuard, AdminGuard,
              { provide: HTTP_INTERCEPTORS, useClass: RequestInterceptor, multi: true}],
  bootstrap: [AppComponent]
})
export class AppModule { }
