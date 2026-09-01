import { Inject, Injectable, Optional } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { API_BASE_URL, GeneralSetup } from './service-proxies';

interface AjaxResponse<T> {
    result: T;
    __abp?: boolean;
}

@Injectable()
export class OutletServiceProxy {
    constructor(private http: HttpClient, @Optional() @Inject(API_BASE_URL) private baseUrl?: string) {}

    getAll(): Observable<GeneralSetup[]> {
        return this.http.get<AjaxResponse<GeneralSetup[]> | GeneralSetup[]>(`${this.baseUrl ?? ''}/api/services/app/GeneralSetup/GetAll`).pipe(
            map((response) => {
                const items = Array.isArray(response) ? response : response?.result;
                return (items || []).map((item) => GeneralSetup.fromJS(item));
            })
        );
    }

    createOrEdit(setup: GeneralSetup): Observable<GeneralSetup> {
        return this.http.post<AjaxResponse<GeneralSetup> | GeneralSetup>(`${this.baseUrl ?? ''}/api/services/app/GeneralSetup/CreateOrEdit`, this.payload(setup)).pipe(
            map((response) => GeneralSetup.fromJS(response && '__abp' in response ? response.result : response))
        );
    }

    delete(id: number): Observable<void> {
        const params = new HttpParams().set('id', id.toString());
        return this.http.delete<void>(`${this.baseUrl ?? ''}/api/services/app/GeneralSetup/Delete`, { params });
    }

    private payload(setup: GeneralSetup): object {
        return {
            id: setup.id,
            serviceCharge: setup.serviceCharge,
            maximumAllowedPercentage: setup.maximumAllowedPercentage,
            monthsBetweenPledgeAndExpiry: setup.monthsBetweenPledgeAndExpiry,
            ticketIdMethod: setup.ticketIdMethod,
            appendedString: setup.appendedString,
            appendYearMonth: setup.appendYearMonth,
            appendedStringBackMethod: setup.appendedStringBackMethod,
            outletName: setup.outletName,
            outletRegistrationNumber: setup.outletRegistrationNumber,
        };
    }
}