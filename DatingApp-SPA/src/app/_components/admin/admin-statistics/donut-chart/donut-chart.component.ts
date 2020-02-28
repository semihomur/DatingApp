import { Component, OnChanges, Input, SimpleChanges } from '@angular/core';

@Component({
  selector: 'app-donut-chart',
  templateUrl: './donut-chart.component.html',
  styleUrls: ['./donut-chart.component.css']
})
export class DonutChartComponent implements OnChanges {
  @Input() label: string;
  @Input() amount: number;
  public doughnutChartLabels = [];
  public doughnutChartData = [];
  public doughnutChartType = 'doughnut';
  constructor() { }

  ngOnChanges(changes: SimpleChanges) {
    this.doughnutChartLabels = [this.label];
    this.amount = Number(changes.amount.currentValue);
    this.doughnutChartData = [this.amount];
  }

}
