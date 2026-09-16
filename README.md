# wex-demo

A small purchase transaction application. Purchases (description, transaction date, USD amount) are stored via a
.NET API and can be retrieved converted into another country's currency using historical exchange rates from the
[Treasury Reporting Rates of Exchange API](https://fiscaldata.treasury.gov/datasets/treasury-reporting-rates-exchange/treasury-reporting-rates-of-exchange).

The repo contains three projects, plus a test project for the API:

| Project | Description |
| --- | --- |
| [`api`](./api) | ASP.NET Core minimal API — stores purchases and returns currency-converted amounts. |
| [`api.Tests`](./api.Tests) | xUnit tests for the API. |
| [`ui-vue`](./ui-vue) | Vue 3 + Vuetify single-page app that consumes the API. |
| [`ui-blazor`](./ui-blazor) | .NET Blazor (Interactive Server) app that consumes the API, mirroring `ui-vue`. |

Requirements for each project are documented in [`Requirements.md`](./Requirements.md),
[`Requirements-UI-vue.md`](./Requirements-UI-vue.md), and [`Requirements-UI-blazor.md`](./Requirements-UI-blazor.md).

## Prerequisites

- [.NET SDK 10.0](https://dotnet.microsoft.com/download) — required for `api`, `api.Tests`, and `ui-blazor`.
- [Node.js](https://nodejs.org/) `^22.18.0` or `>=24.12.0`, with npm — required for `ui-vue`.

The API uses an in-memory database, so no database setup is required. Only one of the two UIs needs to be running
at a time (they're two independent front ends for the same API) — start whichever you want to use.

## Running the API

The API must be running before either UI can create or look up purchases.

```sh
cd api
dotnet run
```

By default it listens on:

- `http://localhost:5274`
- `https://localhost:7283`

An OpenAPI document is available at `/openapi/v1.json` when running in the `Development` environment.

### API endpoints

- `POST /purchases/` — create a purchase. Body: `{ "description": string, "transactionDate": date, "amount": number }`.
- `GET /purchases/{id}?countryCurrency={Country-Currency}` — retrieve a purchase converted to the given currency
  (e.g. `Canada-Dollar`), using the exchange rate for the closest date on or before the transaction date, within
  the last 6 months.

See [`api/api.http`](./api/api.http) for runnable sample requests.

### Running the API tests

```sh
cd api.Tests
dotnet test
```

(You can also run `dotnet test` from the repo root, since [`api.slnx`](./api.slnx) references all three projects.)

## Running the Vue UI (`ui-vue`)

```sh
cd ui-vue
npm install
npm run dev
```

The dev server starts at `http://localhost:5173`. It reads the API's base URL from `ui-vue/.env.development`
(`VITE_API_BASE_URL`, defaults to `http://localhost:5274`) — update it if your API is running elsewhere.

Other useful scripts (run from `ui-vue/`):

```sh
npm run build       # type-check and build for production
npm run test:unit   # run unit tests with Vitest
npm run test:e2e    # run end-to-end tests with Playwright (requires `npx playwright install` first)
npm run lint        # lint with oxlint + eslint
```

## Running the Blazor UI (`ui-blazor`)

```sh
cd ui-blazor
dotnet run
```

By default it listens on:

- `http://localhost:5293`
- `https://localhost:7036`

It reads the API's base URL from `ui-blazor/appsettings.json` (`Api:BaseUrl`, defaults to `http://localhost:5274`) —
update it if your API is running elsewhere.

## Typical local setup

Run the API in one terminal, then either UI in a second terminal:

```sh
# terminal 1
cd api && dotnet run

# terminal 2 (pick one)
cd ui-vue && npm run dev
# or
cd ui-blazor && dotnet run
```

Then open the UI's URL in a browser (`http://localhost:5173` for Vue, `http://localhost:5293` for Blazor) to create
purchases and view them converted to another currency.
