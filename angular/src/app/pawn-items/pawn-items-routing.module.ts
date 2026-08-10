import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PawnItemsComponent } from './pawn-items.component';

const routes: Routes = [
    {
        path: '',
        component: PawnItemsComponent,
        pathMatch: 'full',
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class PawnItemsRoutingModule {}
