import { Component, OnInit } from '@angular/core';
import { trigger, state, style, transition, useAnimation } from '@angular/animations';
import { moveUpCard, moveDownCard } from 'src/app/animation';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-admin-statistics',
  templateUrl: './admin-statistics.component.html',
  styleUrls: ['./admin-statistics.component.css'],
  animations: [
    trigger('divAnimation', [
      state('in', style({
        transform: 'translateY(-20px)'
      })),
      transition('*=>in', [useAnimation(moveUpCard)]),
      transition('in=>*', [useAnimation(moveDownCard)])
    ])
  ]
})
export class AdminStatisticsComponent implements OnInit {
  staticts: Staticts;
  state: any;
  state2: any;
  state3: any;

  constructor(private adminService: AdminService) { }

  ngOnInit() {
    this.adminService.GetStatistics().subscribe((res: Staticts) => {
      this.staticts = res;
    });
  }
  MouseEnterLeave(state: number) {
    switch (state) {
      case 1:
      this.state === 'in' ? this.state = 'out' : this.state = 'in';
      break;
      case 2:
      this.state2 === 'in' ? this.state2 = 'out' : this.state2 = 'in';
      break;
      case 3:
      this.state3 === 'in' ? this.state3 = 'out' : this.state3 = 'in';
      break;
      default:
      break;
    }
  }
}
export interface Staticts {
  totalUser: number;
  totalLikes: number;
  totalPhotoUploaded: number;
}
