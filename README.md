﻿Ratio Shop: personal ecommerce project
===========

Design to solve basic commerce website.
(Not a complete fnished project yet, but the core feature working well)

### Technology used ###

* Run on .net core 6.0
* MS SQL server
* Monolith project, apply repository pattern, use Memory Cache ( has extention to use Distribute cache for further), Cache Tag Helper
* Has Area for admin login, authorization by role, external authentication: google, facebook
* Auto mapper
* Enable session, use authen cookie default, add cartid to cookie
* Customize api key filter attribute (authorizeFilter)
* Predicate builder for filter
* Unit test: MSTest, Moq, FluentAssertions.
* Js: Use ES6 module syntax. (Won't bundle, minify js for production yet)

### Front End ###
* Product listing page: search, filter, sort, paging, preview product variants
* Product: multi variant, multi image, price, discount, stock amount
* Product Detail page, related products
* Package: specific price, package items(multi product variants)
* Cart: list cart items, cart coupons, shipping address, payment gateway design support multi:(currently only support COD)
* Checkout logic, allow return products (stock refresh)
* My Account: user info, shipping info,
* Order History: client manage order history records, view detail
* Login/Register via popup. External authentication: google, facebook
* Header, Footer, Banner, ... content could setting.

### Admin Area ###
* Authentication: use identity, customize info
* Admin Managements:
* Product, Variants, Packages,
* Manage Employee(multi roles),
* SiteSetting ( Header, Footer, PDP, PLP, Slide ....)
* CacheManagement, Clear cache, purge by key
* Order Viewer, Shipment management(list, assign shipment, personalize view by user role: shipper. Shipper could choose order to ship or be assign by admin), Shipment history
* Support server side paging, filter, sort.
* Authorization view by roles

## Store demo ##

Front End | Admin area
----|------
|Account: RatioShopManager/4Bd7U4l8DYR&
[FrontEnd](https://www.ratio-shop.somee.com/)|[Admin](https://www.ratio-shop.somee.com/admin)

### Ratio Shop reference ###

* CozaStore Themes: 
* Login/Registration form Theme:
* Host for production demo: https://somee.com/
