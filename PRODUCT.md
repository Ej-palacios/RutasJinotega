# Product

<!-- impeccable:product-schema 1 -->

## Platform

web

## Users

Travelers — domestic and foreign — exploring tourist routes in San Rafael and its surrounding areas, plus Nicaraguan locals exploring their own region.

Primary job: discover a place worth visiting (known or little-known) and navigate there.

## Product Purpose

Make tourism around San Rafael discoverable and navigable: a Waze-style guide of known and hidden places, with GPS navigation to each.

## Positioning

Known + hidden places around San Rafael, GPS-navigable — a positioning built on local tourism knowledge that a generic maps application could not truthfully copy.

## Operating Context

- Solo/small team; product documentation in Spanish (README.md, docs/ARQUITECTURA.md, docs/DISENO-UI.html).
- Design style baseline: glassmorphism / iOS (see docs/DISENO-UI.html).
- Development phases tracked in docs/ARQUITECTURA.md (roadmap sections 8–11).

## Capabilities and Constraints

- Confirmed MVP scope (verified in code/docs): read-only catalog — municipios, categorias, lugares, detalle, lugares cercanos, street map (Leaflet + OpenStreetMap with schematic fallback); plus a curated San Rafael route page (`/rutas`) from local shops to El Tepeyac.
- Planned capabilities (roadmap): accounts/Identity+JWT, reviews with ratings/photos, favorites, live GPS tracking, offline, community/suggestions (Phases 2–5 of docs/ARQUITECTURA.md).
- Technical constraints: .NET 10 stack (ASP.NET Core API + EF Core/SQL Server, Blazor WebAssembly, MAUI Blazor Hybrid Android, shared Razor Class Library UI); clean-architecture dependency rules (Domain ← Application ← Infrastructure); result pattern instead of exceptions for expected flow; EF migrations as the only schema-change path.
- Explicitly open decisions:
  - Mini pulpería names and coordinates remain examples; Tepeyac, the San Rafael church and the Jinotega cathedral now use verified coordinates and researched history.
  - Map provider (Leaflet/OSM vs Google Maps) and backend/storage host for photos remain open; the client needs internet to fetch tiles.
  - Whether reviews require proof of visit (GPS) is open.

## Evidence on Hand

- docs/ARQUITECTURA.md, docs/DISENO-UI.html, README.md, and the source under src/ and tests/.
- Real, partially verified place data: El Tepeyac, the San Rafael church and the Jinotega cathedral have OSM coordinates and sourced history. Mini pulpería names and coordinates are still examples and must not be presented as verified.

## Product Principles

1. Local tourism knowledge is the asset; software is the vehicle.
2. Known and hidden places both deserve discovery and navigation.
3. Read-only catalog first; social/write capabilities later.
4. Real verified data before scaling.
