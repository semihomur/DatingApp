import { animation, keyframes, animate, style } from '@angular/animations';

export let moveUpCard = animation([
    animate('0.5s', keyframes([
        style({
          transform: 'translateY(0)'
        }),
        style({
          transform: 'translateY(-10px)'
        }),
        style({
          transform: 'translateY(-20px)'
        })
      ]))
]);
export let moveDownCard = animation([
animate('0.5s', keyframes([
    style({
      transform: 'translateY(-20px)'
    }),
    style({
      transform: 'translateY(-10px)'
    }),
    style({
      transform: 'translateY(0)'
    })
  ]))
]);
