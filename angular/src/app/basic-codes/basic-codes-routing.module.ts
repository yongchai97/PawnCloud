import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BasicCodesComponent } from './basic-codes.component';

const routes: Routes = [
    {
        path: '',
        component: BasicCodesComponent,
        data: { title: 'BasicCodes' },
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class BasicCodesRoutingModule {}
