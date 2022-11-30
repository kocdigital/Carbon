# Carbon.ConsoleApplication

Standardization for Console Applications. Add this library and have a working console application immediately with some most commonly used approaches for console applications such as Containerization, Logging, Config Management, Hosting Service etc.

## Add Carbon.ConsoleApplication Support to Your Project
### Get Started

| Out-of-the-box Support                                                           | Availability | Description |
|-----------------------------------------------------------------                 |:----:        |:----:         |
| Kubernetes-enabled (Containerization-enabled)                                    | Yes          | When base helm chart is used as in the [Carbon.Sample Helm Chart](https://github.com/kocdigital/Carbon.Sample/tree/master/helm/CarbonSample-api), it has the best harmony, it simply manages many kubernetes features in one place. Just plug and play.           |
| Logging                                                                          | Yes          | Serilog-based|
| Config Management                                                                | Yes          | Identifies Configmaps when kubernetes used, static config files can be also configured or enables a centralized configuration management via Consul|

**1. Build your API Startup**
