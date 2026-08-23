import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { PawnTicketsRoutingModule } from './pawn-tickets-routing.module';
import { PawnTicketsComponent } from './pawn-tickets.component';
import { CommonModule } from '@angular/common';

@NgModule({
    imports: [
        SharedModule,
        PawnTicketsRoutingModule,
        CommonModule,
        PawnTicketsComponent,
    ],
})
export class PawnTicketsModule {}