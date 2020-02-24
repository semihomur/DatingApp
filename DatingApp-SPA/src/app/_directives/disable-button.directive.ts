import { Directive, ElementRef, Renderer2, HostListener, OnInit, Input } from '@angular/core';

@Directive({
  selector: '[disableButton]'
})
export class DisableButtonDirective implements OnInit {
  @Input() disableButton: number;
  constructor(private elRef: ElementRef, private renderer: Renderer2) { }
  ngOnInit() {
    if (Number(new Date().getTime() / 1000) > Number(localStorage.getItem('approveTime'))) {
      localStorage.removeItem('approveTime');
    } else {
      // tslint:disable-next-line:variable-name
      const numbers = Math.floor((Number(localStorage.getItem('approveTime'))) -  Number((new Date().getTime() / 1000)) );
      this.renderer.setProperty(this.elRef.nativeElement, 'disabled', 'true');
      this.SetInterval(numbers);
    }
  }
  @HostListener('click')
  DisableButton() {
    localStorage.setItem('approveTime', ((new Date().getTime() / 1000) + this.disableButton).toString());
    this.renderer.setProperty(this.elRef.nativeElement, 'disabled', 'true');
    this.SetInterval(this.disableButton);
  }
  SetInterval(numbers) {
    const innerText = this.elRef.nativeElement.innerText;
    const counter = setInterval(() => {
      this.elRef.nativeElement.innerText = numbers;
      numbers--;
      if (numbers < 0) {
        clearInterval(counter);
        this.elRef.nativeElement.innerText = innerText;
        this.elRef.nativeElement.disabled = false;
      }
    }, 1000);
  }
}
