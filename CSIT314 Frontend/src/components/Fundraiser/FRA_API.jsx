import { authFetch } from "./authFetch";

export const fraApi = {
  // GET /api/ViewMyFundraisers
  getMyFundraisers: () =>
    authFetch("/api/ViewMyFundraisers"),

  // GET /api/ViewOneFundraiser?fraId=
  getOne: (fraId) =>
    authFetch(`/api/ViewOneFundraiser?fraId=${fraId}`),

  // GET /api/SearchFundraiser?name=
  search: (name) =>
    authFetch(`/api/SearchFundraiser?name=${encodeURIComponent(name)}`),

  // POST /api/CreateFundraiser
  create: (body) =>
    authFetch("/api/CreateFundraiser", {
      method: "POST",
      body: JSON.stringify(body),
    }),

  // PUT /api/UpdateFundraiser
  update: (body) =>
    authFetch("/api/UpdateFundraiser", {
      method: "PUT",
      body: JSON.stringify(body),
    }),

  // DELETE /api/DeleteFundraiser?fundraiserId=
  delete: (fundraiserId) =>
    authFetch(`/api/DeleteFundraiser?fundraiserId=${fundraiserId}`, {
      method: "DELETE",
    }),

  // GET /api/Category (adjust route if different)
  getCategories: () =>
    authFetch("/api/Category"),
};