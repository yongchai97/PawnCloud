import { NgModule } from '@angular/core';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AbpHttpInterceptor } from 'abp-ng2-module';

import * as ApiServiceProxies from './service-proxies';
import { LookupServiceProxy } from './lookup-service-proxy';

@NgModule({
    providers: [
        ApiServiceProxies.RoleServiceProxy,
        ApiServiceProxies.SessionServiceProxy,
        ApiServiceProxies.TenantServiceProxy,
        ApiServiceProxies.UserServiceProxy,
        ApiServiceProxies.TokenAuthServiceProxy,
        ApiServiceProxies.AccountServiceProxy,
        ApiServiceProxies.ConfigurationServiceProxy,
        ApiServiceProxies.CustomerServiceProxy,
        ApiServiceProxies.CustomerDocumentServiceProxy,
        ApiServiceProxies.PawnTicketServiceProxy,
        ApiServiceProxies.PawnItemServiceProxy,
        ApiServiceProxies.LoanServiceProxy,
        ApiServiceProxies.MiscFunctionServiceProxy,
        LookupServiceProxy,
        { provide: HTTP_INTERCEPTORS, useClass: AbpHttpInterceptor, multi: true },
    ],
})
export class ServiceProxyModule {}