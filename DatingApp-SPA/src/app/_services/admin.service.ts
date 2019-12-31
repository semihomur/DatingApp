import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
baseUrl = environment.apiUrl;
constructor(private http: HttpClient) { }
getUsersWithRoles() {
  return this.http.get(this.baseUrl + 'admin/usersWithRoles');
}
updateUserRoles(user: User, roles: {}) {
  return this.http.post(this.baseUrl + 'admin/editRoles/' + user.userName, roles);
}
getNotApprovedPhotos() {
  return this.http.get(this.baseUrl + 'admin/photosForModerations');
}
deletePhoto(photoid: number) {
  return this.http.delete(this.baseUrl + 'admin/deletePhoto/' + photoid);
}
approvePhoto(photoid: number) {
  return this.http.post(this.baseUrl + 'admin/approvePhoto/' + photoid, photoid);
}
}
