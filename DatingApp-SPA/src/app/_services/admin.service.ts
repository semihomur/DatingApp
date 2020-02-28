import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
baseUrl = environment.apiUrl + 'admin/';
constructor(private http: HttpClient) { }
getUsersWithRoles() {
  return this.http.get(this.baseUrl + 'usersWithRoles');
}
updateUserRoles(user: User, roles: {}) {
  return this.http.post(this.baseUrl + 'editRoles/' + user.userName, roles);
}
getNotApprovedPhotos() {
  return this.http.get(this.baseUrl + 'photosForModerations');
}
deletePhoto(photoid: number) {
  return this.http.delete(this.baseUrl + 'deletePhoto/' + photoid);
}
approvePhoto(photoid: number) {
  return this.http.post(this.baseUrl + 'approvePhoto/' + photoid, photoid);
}
getReports() {
  return this.http.get(this.baseUrl + 'getReports');
}
MakeInactive(reportId: number) {
  return this.http.put(this.baseUrl + 'inActivateUser/' + reportId, {});
}
GetStatistics() {
  return this.http.get(this.baseUrl + 'getStatistics');
}
}
