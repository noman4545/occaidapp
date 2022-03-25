import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Login } from "../models/login.model";
import { User } from "../models/user.model";
import { take } from 'rxjs/operators';
import { Observable } from "rxjs";
import { Token } from "../models/token.model";
import { Users } from "../models/users.model";
import { NewPassword } from "../models/newPassword.model";

@Injectable()
export class AuthenticationService {
    API_URL:string = 'api/Auth/';
    constructor(private http: HttpClient){}

    loginProcess(user: Login): Observable<Token>{
        return this.http.post<Token>(this.API_URL + 'Login', user, {responseType: 'json'}).pipe(take(1));
    }

    signupProcess(user: User): Observable<boolean>{
        return this.http.post<boolean>(this.API_URL + 'Register', user, {responseType: 'json'}).pipe(take(1));
    }

    verifyLoginToken(token: string): Observable<boolean>{
        return this.http.get<boolean>(this.API_URL + 'VerifyLoginToken',
        {responseType: 'json', headers: new HttpHeaders().set('Authorization', `Bearer ${token}`)}).pipe(take(1));
    }

    verifyAdminToken(token: string): Observable<boolean>{
        return this.http.get<boolean>(this.API_URL + 'VerifyAdminToken',
        {responseType: 'json', headers: new HttpHeaders().set('Authorization', `Bearer ${token}`)}).pipe(take(1));
    }

    getAllUser(): Observable<Users[]>{
        return this.http.get<Users[]>(this.API_URL + 'GetAllUsers', {responseType: 'json'}).pipe(take(1));
    }

    deleteUser(userId: string): Observable<boolean>{
        return this.http.get<boolean>(this.API_URL + 'DeleteUsers?userid=' + userId, {responseType: 'json'}).pipe(take(1));
    }

    setNewPassword(passwordData: NewPassword): Observable<boolean>{
        return this.http.post<boolean>(this.API_URL + 'SetNewPassword', passwordData).pipe(take(1));
    }
}