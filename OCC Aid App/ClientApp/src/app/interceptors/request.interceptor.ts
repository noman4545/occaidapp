import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { Token } from "src/app/models/token.model";

@Injectable()
export class RequestInterceptor implements HttpInterceptor{
    constructor(){
    }
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
      let tokenData: Token = JSON.parse(localStorage.getItem('ASPNetAuthToken') as string) as Token;
      if(tokenData != null){
        req = req.clone({
          setHeaders: {
            Authorization: `Bearer ${tokenData.token}`
          }
        });
      }
      return next.handle(req);
    }
}