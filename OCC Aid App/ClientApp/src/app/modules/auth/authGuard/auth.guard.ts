import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, CanActivate, NavigationEnd, Router, RouterStateSnapshot } from "@angular/router";
import { Observable, of } from "rxjs";
import { AuthenticationService } from "../../../services/authentication.service";
import { Token } from "../../../models/token.model";

@Injectable()
export class AuthGuard implements CanActivate{

    constructor(private router: Router,
        private authenticationService: AuthenticationService) {}

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
        return new Observable<boolean>(observe => {
            let tokenData: Token = JSON.parse(localStorage.getItem('ASPNetAuthToken') as string) as Token;
            if(tokenData != null){
                this.authenticationService.verifyLoginToken(tokenData.token).subscribe(res => {
                    if(res){
                        return observe.next(true);
                    }
                    localStorage.clear();
                    this.router.navigate(['/auth']);
                    return observe.next(false);
                }
                ,err => {
                    localStorage.clear();
                    this.router.navigate(['/auth']);
                    return observe.next(false);
                });
            }
            else{
                this.router.navigate(['/auth']);
                return observe.next(false);
            }
        });
    }
}