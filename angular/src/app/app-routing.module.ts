import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';
import { AppComponent } from './app.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: AppComponent,
                children: [
                    {
                        path: '',
                        redirectTo: 'home',
                        pathMatch: 'full',
                    },
                    {
                        path: 'home',
                        loadChildren: () => import('./home/home.module').then((m) => m.HomeModule),
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'customers',
                        loadChildren: () => import('./customers/customers.module').then((m) => m.CustomersModule),
                        data: { permission: 'Pages.Customers' },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'users',
                        loadChildren: () => import('./users/users.module').then((m) => m.UsersModule),
                        data: { permission: 'Pages.Users' },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'roles',
                        loadChildren: () => import('./roles/roles.module').then((m) => m.RolesModule),
                        data: { permission: 'Pages.Roles' },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'tenants',
                        loadChildren: () => import('./tenants/tenants.module').then((m) => m.TenantsModule),
                        data: { permission: 'Pages.Tenants' },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'pawn-tickets',
                        loadChildren: () => import('./pawn-tickets/pawn-tickets.module').then((m) => m.PawnTicketsModule),
                        data: { permission: 'Pages.PawnTickets' },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'pawn-items',
                        loadChildren: () => import('./pawn-items/pawn-items.module').then((m) => m.PawnItemsModule),
                        data: { permission: 'Pages.PawnItems' },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'loans',
                        loadChildren: () => import('./loans/loans.module').then((m) => m.LoansModule),
                        data: { permission: 'Pages.Loans' },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'basic-codes',
                        loadChildren: () => import('./basic-codes/basic-codes.module').then((m) => m.BasicCodesModule),
                        data: { permission: 'Pages.BasicCodes' },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'admin',
                        children: [
                            {
                                path: 'gold-types',
                                loadChildren: () => import('./admin/gold-types/gold-types.module').then((m) => m.GoldTypesModule),
                                data: { permission: 'Pages.GoldTypes' },
                                canActivate: [AppRouteGuard],
                            },
                            {
                                path: 'gold-price-entry',
                                loadChildren: () => import('./admin/gold-price-entry/gold-price-entry.module').then((m) => m.GoldPriceEntryModule),
                                canActivate: [AppRouteGuard],
                            },
                            {
                                path: 'seeder',
                                loadChildren: () => import('./admin/seeder/seeder.module').then((m) => m.SeederModule),
                                canActivate: [AppRouteGuard],
                            },
                        ],
                    },
                    {
                        path: 'update-password',
                        loadChildren: () => import('./users/users.module').then((m) => m.UsersModule),
                        canActivate: [AppRouteGuard],
                    },
                ],
            },
        ]),
    ],
    exports: [RouterModule],
})
export class AppRoutingModule {}