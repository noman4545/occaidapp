import { AuthModule } from './../auth/auth.module';
import { NgModule } from '@angular/core';
import { PreloadAllModules, RouterModule, Routes } from '@angular/router';
import { ACIDComponent } from './components/acid/acid.component';
import { CentralComponent } from './components/central/central.component';
import { ErrorComponent } from './components/error/error.component';
import { IOSCodesComponent } from './components/ioscodes/ioscodes.component';
import { MainComponent } from './components/main/main.component';
import { SMSComponent } from './components/sms/sms.component';
import { TMCSComponent } from './components/tmcs/tmcs.component';
import { AuthGuard } from '../auth/authGuard/auth.guard';
import { TMCSLatestComponent } from './components/tmcs-latest/tmcs-latest.component';

const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'auth' },
  {
    path: 'main', component: MainComponent, children: [
      { path: '', pathMatch: 'full', redirectTo: 'ios-codes' },
      { path: 'ios-codes', component: IOSCodesComponent, canActivate: [AuthGuard] },
      { path: 'acid-cctv', component: ACIDComponent, canActivate: [AuthGuard] },
      { path: 'tmcs', component: TMCSComponent, canActivate: [AuthGuard] },
      { path: 'tmcs-latest', component: TMCSLatestComponent, canActivate: [AuthGuard] },
      { path: 'sms', component: SMSComponent, canActivate: [AuthGuard] },
    ]
  },
  { path: 'central', component: CentralComponent, canActivate: [AuthGuard] },
  { path: 'admin-controls', loadChildren: () => import('../admin/admin.module').then(m => m.AdminModule) },
  { path: 'auth', loadChildren: () => import('../auth/auth.module').then(m => m.AuthModule) },
  { path: '**', component: ErrorComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes,
    {
      preloadingStrategy: PreloadAllModules
    })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
