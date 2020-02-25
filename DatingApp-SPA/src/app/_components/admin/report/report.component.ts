import { Component, OnInit } from '@angular/core';
import { AdminService } from 'src/app/_services/admin.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-report',
  templateUrl: './report.component.html',
  styleUrls: ['./report.component.css']
})
export class ReportComponent implements OnInit {
  reports = [];
  constructor(private adminService: AdminService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.adminService.getReports().subscribe((reports: any) => {
        this.reports = reports;
    }, () => {
      this.alertify.error('Could not take reports');
    });
  }
  InActivate(reportId: number) {
    this.adminService.MakeInactive(reportId).subscribe(() => {
      this.reports.splice(this.reports.findIndex(r => r.id === reportId), 1);
      this.alertify.success('Succesfull');
    }, () => this.alertify.error('Something went wrong'));
  }

}
