import { Inject, Injectable, Optional } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { API_BASE_URL } from './service-proxies';

export interface AuditLogDto {
    id: number;
    tenantId?: number;
    userId?: number;
    userName?: string;
    serviceName?: string;
    methodName?: string;
    parameters?: string;
    returnValue?: string;
    executionTime?: string;
    executionDuration: number;
    clientIpAddress?: string;
    clientName?: string;
    browserInfo?: string;
    exceptionMessage?: string;
}

export interface AuditLogPagedResultDto {
    totalCount: number;
    items: AuditLogDto[];
}

@Injectable()
export class AuditLogServiceProxy {
    constructor(
        private http: HttpClient,
        @Optional() @Inject(API_BASE_URL) private baseUrl?: string
    ) {}

    getAll(filter: string, skipCount: number, maxResultCount: number): Observable<AuditLogPagedResultDto> {
        const params = new HttpParams()
            .set('Filter', filter || '')
            .set('SkipCount', skipCount)
            .set('MaxResultCount', maxResultCount);

        return this.http
            .get<any>(`${this.baseUrl ?? ''}/api/services/app/AuditLog/GetAll`, { params })
            .pipe(map((response) => (response && response.__abp ? response.result : response)));
    }
}
