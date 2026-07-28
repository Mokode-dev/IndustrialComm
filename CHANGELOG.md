# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2026-07-28

### Added

- `IndustrialComm.Core` foundation package for the IndustrialComm ecosystem.
- Binary helpers: endian-aware readers/writers, bit packing, BCD conversion.
- Checksums: CRC-16/Modbus, CRC-16/CCITT-FALSE, CRC-32, LRC.
- Framing: ring frame buffer and pluggable frame detectors (fixed length, length-prefix, delimiter).
- Transport: `IByteTransport` and `StreamByteTransport`.
- Results: `CommResult` / `CommResult<T>` and `CommError` / `CommErrorCode`.
- Reliability: timeout gates and configurable retry policies.
- Diagnostics: null and counting diagnostics sinks.
- Primitives: data quality, timestamps, and engineering units helpers.
