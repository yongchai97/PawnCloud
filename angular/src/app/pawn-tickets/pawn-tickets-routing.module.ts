import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { PawnTicketsComponent } from './pawn-tickets.component';

@NgModule({
    imports: [RouterModule.forChild([{ path: '', component: PawnTicketsComponent }])],
    exports: [RouterModule],
})
export class PawnTicketsRoutingModule {}
