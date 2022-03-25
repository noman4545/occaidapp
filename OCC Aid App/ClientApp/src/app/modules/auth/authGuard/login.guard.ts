import { Injectable } from "@angular/core";
import { CanActivate, Router } from "@angular/router";
import { Observable, of } from "rxjs";
import { catchError, map } from "rxjs/operators";
import { AuthenticationService } from "../../../services/authentication.service";
import { Token } from "../../../models/token.model";

@Injectable()
export class LoginGuard implements CanActivate{

    constructor(private router: Router, 
        private authenticationService: AuthenticationService) {}

    canActivate(): Observable<boolean> {
        return new Observable(observe => {
            let tokenData: Token = JSON.parse(localStorage.getItem('ASPNetAuthToken') as string) as Token;
            if(tokenData != null){
                this.authenticationService.verifyLoginToken(tokenData.token).subscribe(res => {
                    if(tokenData.role.toLowerCase() == 'admin'){
                        this.router.navigate(['admin-controls']);
                    }
                    else{
                        this.router.navigate(['central']);
                    }
                    return observe.next(false);
                },
                err => {
                    return observe.next(true);
                });
            }
            else{
                return observe.next(true);
            }
        });
    }
}