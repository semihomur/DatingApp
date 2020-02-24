import { Directive, ElementRef, Renderer2, HostListener, OnInit } from '@angular/core';

@Directive({
  selector: '[disableButton]'
})
export class DisableButtonDirective implements OnInit {
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
    const numbers = 20;
    localStorage.setItem('approveTime', ((new Date().getTime() / 1000) + numbers).toString());
    this.renderer.setProperty(this.elRef.nativeElement, 'disabled', 'true');
    this.SetInterval(numbers);
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
