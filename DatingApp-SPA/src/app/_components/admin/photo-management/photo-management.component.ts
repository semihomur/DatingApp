import { Component, OnInit } from '@angular/core';
import { AdminService } from 'src/app/_services/admin.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Photo } from 'src/app/_models/Photo';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {
  photos: Photo[];
  constructor(private adminService: AdminService , private alertify: AlertifyService) { }

  ngOnInit() {
    this.getNotApprovedPhotos();
  }
  getNotApprovedPhotos() {
    this.adminService.getNotApprovedPhotos().subscribe((res: Photo[]) => {
      this.photos = res;
    }, error => this.alertify.error(error.error));
  }
  DeletePhoto(photoid: number) {
    this.adminService.deletePhoto(photoid).subscribe(() => {
      this.alertify.success('Photo is deleted');
      this.photos = this.photos.filter(i => i.id !== photoid);
    }, error => this.alertify.error(error.error));
  }
  ApprovePhoto(photoid: number) {
    this.adminService.approvePhoto(photoid).subscribe(() => {
      this.alertify.success('Photo is approved');
      this.photos = this.photos.filter(i => i.id !== photoid);
    }, error => this.alertify.error(error.error));
  }

}
