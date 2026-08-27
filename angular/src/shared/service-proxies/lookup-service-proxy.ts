import { Injectable, Inject, Optional } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { API_BASE_URL, BasicCodeLookupDto, MiscMasterConfigLookupDto } from './service-proxies';

/**
 * ABP wraps every DynamicWebApi response in an AjaxResponse envelope:
 *   { result: <payload>, success: true, error: null, __abp: true, ... }
 *
 * The AbpHttpInterceptor is supposed to unwrap this for generated NSwag
 * proxies, but our hand-rolled http.get calls below bypass that path and
 * receive the envelope as-is. unwrapAjaxResponse() handles both shapes
 * (already-unwrapped, or still-wrapped) so callers always see the payload.
 */
export interface AjaxResponse<T> {
    result: T;
    targetUrl: string | null;
    success: boolean;
    error: any;
    unAuthorizedRequest: boolean;
    __abp: boolean;
}

function unwrapAjaxResponse<T>(source: Observable<any>): Observable<T> {
    return source.pipe(
        map((response) => {
            if (response && typeof response === 'object' && '__abp' in response && 'result' in response) {
                return (response as AjaxResponse<T>).result as T;
            }
            return response as T;
        })
    );
}

export class CustomerLookupDto {
    id: number = 0;
    displayName: string = '';
    customerNo: string = '';
    fullName: string = '';
    nric: string = '';
}

export class PawnTicketLookupDto {
    id: number = 0;
    displayName: string = '';
}

export class CustomerDocumentSummaryDto {
    id: number = 0;
    customer: number = 0;
    documentType?: string;
    fileName?: string;
    creationTime?: string;
}

export class LookupListResultDto<T> {
    items: T[] = [];
}

@Injectable()
export class LookupServiceProxy {
    constructor(
        private http: HttpClient,
        @Optional() @Inject(API_BASE_URL) private baseUrl?: string
    ) {}

    private url(path: string): string {
        return (this.baseUrl ?? '') + '/api/services/app' + path;
    }

    getCustomersForLookup(filter?: string): Observable<LookupListResultDto<CustomerLookupDto>> {
        let params = new HttpParams();
        if (filter) {
            params = params.set('Filter', filter);
        }
        return unwrapAjaxResponse<LookupListResultDto<CustomerLookupDto>>(
            this.http.get(this.url('/Customer/GetCustomersForLookup'), { params })
        );
    }

    getBasicCodesByCategory(categoryId: number): Observable<LookupListResultDto<BasicCodeLookupDto>> {
        const params = new HttpParams().set('CategoryId', categoryId);
        return unwrapAjaxResponse<LookupListResultDto<BasicCodeLookupDto>>(
            this.http.get(this.url('/BasicCode/GetBasicCodesByCategory'), { params })
        );
    }

    getMiscMasterConfigs(): Observable<LookupListResultDto<MiscMasterConfigLookupDto>> {
        return unwrapAjaxResponse<LookupListResultDto<MiscMasterConfigLookupDto>>(
            this.http.get(this.url('/MiscMasterConfig/GetForLookup'))
        );
    }

    getPawnTicketsForLookup(): Observable<LookupListResultDto<PawnTicketLookupDto>> {
        return unwrapAjaxResponse<LookupListResultDto<PawnTicketLookupDto>>(
            this.http.get(this.url('/PawnTicket/GetPawnTicketsForLookup'))
        );
    }

    /**
     * Hand-rolled listing endpoint for the Pawn Tickets page.
     *
     * The NSwag-generated PawnTicketServiceProxy.getAll() uses
     * responseType: "blob" with Accept: "text/plain", which causes the
     * AbpHttpInterceptor to skip envelope unwrapping (its Blob branch
     * only fires when the Blob MIME type contains "application/json").
     * The result is that processGetAll() receives the still-wrapped
     * AjaxResponse and fromJS() produces an empty paged DTO. Going
     * through HttpClient.get() directly keeps the response as a plain
     * JSON object, so the interceptor unwraps it normally.
     */
    getPawnTickets(
        filter: string | undefined,
        sorting: string | undefined,
        skipCount: number | undefined,
        maxResultCount: number | undefined
    ): Observable<any> {
        let params = new HttpParams();
        if (filter !== undefined && filter !== null) {
            params = params.set('Filter', filter);
        }
        if (sorting !== undefined && sorting !== null) {
            params = params.set('Sorting', sorting);
        }
        if (skipCount !== undefined && skipCount !== null) {
            params = params.set('SkipCount', skipCount.toString());
        }
        if (maxResultCount !== undefined && maxResultCount !== null) {
            params = params.set('MaxResultCount', maxResultCount.toString());
        }
        return unwrapAjaxResponse<any>(
            this.http.get(this.url('/PawnTicket/GetAll'), { params })
        );
    }

    deletePawnTicket(id: number): Observable<void> {
        const params = new HttpParams().set('Id', id.toString());
        return unwrapAjaxResponse<void>(
            this.http.delete(this.url('/PawnTicket/Delete'), { params })
        );
    }

    getCustomerDocumentsByCustomerId(customerId: number): Observable<LookupListResultDto<CustomerDocumentSummaryDto>> {
        const params = new HttpParams().set('Id', customerId.toString());
        return unwrapAjaxResponse<LookupListResultDto<CustomerDocumentSummaryDto>>(
            this.http.get(this.url('/CustomerDocument/GetByCustomerId'), { params })
        );
    }

    // Backend CreateOrEdit now returns the entity id (int).
    // The NSwag-generated proxies still declare createOrEdit as Observable<void>, so these
    // wrappers re-issue the same POST and parse the int response.
    createCustomer(input: any): Observable<number> {
        return this.http.post<number>(this.url('/Customer/CreateOrEdit'), input);
    }

    createCustomerDocument(input: any): Observable<number> {
        return this.http.post<number>(this.url('/CustomerDocument/CreateOrEdit'), input);
    }

    createPawnTicket(input: any): Observable<number> {
        return this.http.post<number>(this.url('/PawnTicket/CreateOrEdit'), input);
    }

    createLoan(input: any): Observable<number> {
        return this.http.post<number>(this.url('/Loan/CreateOrEdit'), input);
    }

}