# Carbon.TimeSeriesDb.Abstractions

> TimescaleDB is an open-source database designed to make SQL scalable for time-series data. 
> It is engineered up from PostgreSQL and packaged as a PostgreSQL extension, 
> providing automatic partitioning across time and space (partitioning key), as well as full SQL support.[**]

[**] https://github.com/timescale/timescaledb

This package brings timeserie database specific abstractions and a base package for [Carbon.TimeScaleDb.EntityFrameworkCore](../Carbon.TimeScaleDb.EntityFrameworkCore/README.Md) 
Using this package as stand-alone is not *recommended*. Please use the mentioned one in order to enable more capabilities and numerous type of operations set.

Any other timeserie databases other than TimeScaleDb will be introduced by implementing this abstraction library.
