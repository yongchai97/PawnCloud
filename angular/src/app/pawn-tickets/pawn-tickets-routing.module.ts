import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PawnTicketsComponent } from './pawn-tickets.component';

const routes: Routes = [
    {
        path: '',
        component: PawnTicketsComponent,
        pathMatch: 'full',
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class PawnTicketsRoutingModule {}