import { Injectable } from "@angular/core";
import { CanActivate, Router } from "@angular/router";
import { Observable, of } from "rxjs";
import { AuthenticationService } from "../../../services/authentication.service";
import { Token } from "../../../models/token.model";

@Injectable()
export class AdminGuard implements CanActivate{

    constructor(private router: Router, 
        private authenticationService: AuthenticationService) {}

    canActivate(): Observable<boolean> {
        return new Observable<boolean>(observe => {
            let tokenData: Token = JSON.parse(localStorage.getItem('ASPNetAuthToken') as string) as Token;
            if(tokenData != null){
                this.authenticationService.verifyAdminToken(tokenData.token).subscribe(res => {
                    if(res)
                        return observe.next(true);
                    this.router.navigate(['/auth']);
                    return observe.next(false);
                }
                ,err => {
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