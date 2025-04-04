# Monitoring and logging

## Overview dashboard

```json
{
  "OwnerId": null,
  "Title": "Overview",
  "IsProtected": false,
  "SignalExpression": null,
  "Charts": [
    {
      "Id": "chart-6",
      "Title": "All Events",
      "SignalExpression": null,
      "Queries": [
        {
          "Id": "chartquery-7",
          "Measurements": [
            {
              "Value": "count(*)",
              "Label": "count"
            }
          ],
          "Where": null,
          "SignalExpression": null,
          "GroupBy": [],
          "DisplayStyle": {
            "Type": "Line",
            "LineFillToZeroY": true,
            "LineShowMarkers": false,
            "BarOverlaySum": false,
            "SuppressLegend": false,
            "Palette": "Default"
          },
          "Having": null,
          "OrderBy": [],
          "Limit": null
        }
      ],
      "DisplayStyle": {
        "WidthColumns": 8,
        "HeightRows": 1
      }
    },
    {
      "Id": "chart-8",
      "Title": "Count by Level",
      "SignalExpression": null,
      "Queries": [
        {
          "Id": "chartquery-9",
          "Measurements": [
            {
              "Value": "count(*)",
              "Label": "count"
            }
          ],
          "Where": null,
          "SignalExpression": null,
          "GroupBy": [
            "@Level"
          ],
          "DisplayStyle": {
            "Type": "Pie",
            "LineFillToZeroY": false,
            "LineShowMarkers": true,
            "BarOverlaySum": false,
            "SuppressLegend": false,
            "Palette": "Default"
          },
          "Having": null,
          "OrderBy": [],
          "Limit": null
        }
      ],
      "DisplayStyle": {
        "WidthColumns": 4,
        "HeightRows": 1
      }
    },
    {
      "Id": "chart-10",
      "Title": "Errors and Exceptions",
      "SignalExpression": null,
      "Queries": [
        {
          "Id": "chartquery-11",
          "Measurements": [
            {
              "Value": "count(*)",
              "Label": "count"
            }
          ],
          "Where": "@Exception is not null or @Level in ['f', 'fa', 'fat', 'ftl', 'fata', 'fatl', 'fatal', 'c', 'cr', 'cri', 'crt', 'crit', 'critical', 'alert', 'emerg', 'panic', 'e', 'er', 'err', 'eror', 'erro', 'error'] ci",
          "SignalExpression": null,
          "GroupBy": [],
          "DisplayStyle": {
            "Type": "Bar",
            "LineFillToZeroY": false,
            "LineShowMarkers": true,
            "BarOverlaySum": true,
            "SuppressLegend": false,
            "Palette": "Reds"
          },
          "Having": null,
          "OrderBy": [],
          "Limit": null
        }
      ],
      "DisplayStyle": {
        "WidthColumns": 8,
        "HeightRows": 1
      }
    },
    {
      "Id": "chart-12",
      "Title": "Distinct Event Types",
      "SignalExpression": null,
      "Queries": [
        {
          "Id": "chartquery-13",
          "Measurements": [
            {
              "Value": "count(distinct(@EventType))",
              "Label": "count"
            }
          ],
          "Where": null,
          "SignalExpression": null,
          "GroupBy": [],
          "DisplayStyle": {
            "Type": "Value",
            "LineFillToZeroY": false,
            "LineShowMarkers": true,
            "BarOverlaySum": false,
            "SuppressLegend": false,
            "Palette": "Default"
          },
          "Having": null,
          "OrderBy": [],
          "Limit": null
        }
      ],
      "DisplayStyle": {
        "WidthColumns": 4,
        "HeightRows": 1
      }
    }
  ],
  "Id": "dashboard-14",
  "Links": {
    "Self": "api/dashboards/dashboard-14?version=1",
    "Group": "api/dashboards/resources"
  }
}
```

## Endpoint dashboard

```json
{
  "OwnerId": "user-admin",
  "Title": "Endpoint overview",
  "IsProtected": false,
  "SignalExpression": null,
  "Charts": [
    {
      "Id": "chart-19",
      "Title": "Post UserMessage Count",
      "SignalExpression": null,
      "Queries": [
        {
          "Id": "chartquery-20",
          "Measurements": [
            {
              "Value": "count(*)",
              "Label": "count"
            }
          ],
          "Where": "RequestMethod = 'POST' and RequestPath like '/msgs/%'",
          "SignalExpression": null,
          "GroupBy": [],
          "DisplayStyle": {
            "Type": "Line",
            "LineFillToZeroY": false,
            "LineShowMarkers": true,
            "BarOverlaySum": false,
            "SuppressLegend": false,
            "Palette": "Reds"
          },
          "Having": null,
          "OrderBy": [],
          "Limit": 10000
        }
      ],
      "DisplayStyle": {
        "WidthColumns": 6,
        "HeightRows": 1
      }
    },
    {
      "Id": "chart-22",
      "Title": "Get UserMessage Count",
      "SignalExpression": null,
      "Queries": [
        {
          "Id": "chartquery-23",
          "Measurements": [
            {
              "Value": "count(*)",
              "Label": "Count"
            }
          ],
          "Where": "RequestMethod = 'GET' and RequestPath like '/msgs/%'",
          "SignalExpression": null,
          "GroupBy": [],
          "DisplayStyle": {
            "Type": "Line",
            "LineFillToZeroY": false,
            "LineShowMarkers": true,
            "BarOverlaySum": false,
            "SuppressLegend": false,
            "Palette": "Reds"
          },
          "Having": null,
          "OrderBy": [],
          "Limit": 100
        }
      ],
      "DisplayStyle": {
        "WidthColumns": 6,
        "HeightRows": 1
      }
    },
    {
      "Id": "chart-28",
      "Title": "Post Message Elapsed Time",
      "SignalExpression": null,
      "Queries": [
        {
          "Id": "chartquery-29",
          "Measurements": [
            {
              "Value": "min(Elapsed)",
              "Label": "MinElapsed"
            },
            {
              "Value": "max(Elapsed)",
              "Label": "MaxElapsed"
            },
            {
              "Value": "percentile(Elapsed, 50)",
              "Label": "MedianElapsed"
            },
            {
              "Value": "percentile(Elapsed, 90)",
              "Label": "P90Elapsed"
            },
            {
              "Value": "mean(Elapsed)",
              "Label": "MeanElapsed"
            }
          ],
          "Where": "RequestMethod = 'POST' and RequestPath like '/msgs%'",
          "SignalExpression": null,
          "GroupBy": [],
          "DisplayStyle": {
            "Type": "Bar",
            "LineFillToZeroY": false,
            "LineShowMarkers": true,
            "BarOverlaySum": false,
            "SuppressLegend": false,
            "Palette": "Default"
          },
          "Having": null,
          "OrderBy": [],
          "Limit": 10000
        }
      ],
      "DisplayStyle": {
        "WidthColumns": 6,
        "HeightRows": 1
      }
    },
    {
      "Id": "chart-30",
      "Title": "Get Message Elapsed Time",
      "SignalExpression": null,
      "Queries": [
        {
          "Id": "chartquery-31",
          "Measurements": [
            {
              "Value": "min(Elapsed)",
              "Label": "MinElapsed"
            },
            {
              "Value": "max(Elapsed)",
              "Label": "MaxElapsed"
            },
            {
              "Value": "percentile(Elapsed, 50)",
              "Label": "MedianElapsed"
            },
            {
              "Value": "percentile(Elapsed, 90)",
              "Label": "P90Elapsed"
            },
            {
              "Value": "mean(Elapsed)",
              "Label": "MeanElapsed"
            }
          ],
          "Where": "RequestMethod = 'GET' and RequestPath like '/msgs%'",
          "SignalExpression": null,
          "GroupBy": [],
          "DisplayStyle": {
            "Type": "Bar",
            "LineFillToZeroY": false,
            "LineShowMarkers": true,
            "BarOverlaySum": false,
            "SuppressLegend": false,
            "Palette": "Default"
          },
          "Having": null,
          "OrderBy": [],
          "Limit": 10000
        }
      ],
      "DisplayStyle": {
        "WidthColumns": 6,
        "HeightRows": 1
      }
    },
    {
      "Id": "chart-32",
      "Title": "Post Follow Count",
      "SignalExpression": null,
      "Queries": [
        {
          "Id": "chartquery-33",
          "Measurements": [
            {
              "Value": "count(*)",
              "Label": "count"
            }
          ],
          "Where": "RequestMethod = 'POST' and RequestPath like '/fllws%'",
          "SignalExpression": null,
          "GroupBy": [],
          "DisplayStyle": {
            "Type": "Line",
            "LineFillToZeroY": false,
            "LineShowMarkers": true,
            "BarOverlaySum": false,
            "SuppressLegend": false,
            "Palette": "Greens"
          },
          "Having": null,
          "OrderBy": [],
          "Limit": 10000
        }
      ],
      "DisplayStyle": {
        "WidthColumns": 6,
        "HeightRows": 1
      }
    },
    {
      "Id": "chart-34",
      "Title": "Get Follow Count",
      "SignalExpression": null,
      "Queries": [
        {
          "Id": "chartquery-35",
          "Measurements": [
            {
              "Value": "count(*)",
              "Label": "count"
            }
          ],
          "Where": "RequestMethod = 'GET' and RequestPath like '/fllws%'",
          "SignalExpression": null,
          "GroupBy": [],
          "DisplayStyle": {
            "Type": "Line",
            "LineFillToZeroY": false,
            "LineShowMarkers": true,
            "BarOverlaySum": false,
            "SuppressLegend": false,
            "Palette": "Greens"
          },
          "Having": null,
          "OrderBy": [],
          "Limit": null
        }
      ],
      "DisplayStyle": {
        "WidthColumns": 6,
        "HeightRows": 1
      }
    },
    {
      "Id": "chart-36",
      "Title": "Post Follow Elapsed Time",
      "SignalExpression": null,
      "Queries": [
        {
          "Id": "chartquery-37",
          "Measurements": [
            {
              "Value": "min(Elapsed)",
              "Label": "MinElapsed"
            },
            {
              "Value": "max(Elapsed)",
              "Label": "MaxElapsed"
            },
            {
              "Value": "percentile(Elapsed, 50)",
              "Label": "MedianElapsed"
            },
            {
              "Value": "percentile(Elapsed, 90)",
              "Label": "P90Elapsed"
            },
            {
              "Value": "mean(Elapsed)",
              "Label": "MeanElapsed"
            }
          ],
          "Where": "RequestMethod = 'POST' and RequestPath like '/fllws%'",
          "SignalExpression": null,
          "GroupBy": [],
          "DisplayStyle": {
            "Type": "Bar",
            "LineFillToZeroY": false,
            "LineShowMarkers": true,
            "BarOverlaySum": false,
            "SuppressLegend": false,
            "Palette": "Default"
          },
          "Having": null,
          "OrderBy": [],
          "Limit": 10000
        }
      ],
      "DisplayStyle": {
        "WidthColumns": 6,
        "HeightRows": 1
      }
    },
    {
      "Id": "chart-38",
      "Title": "Get Follow Elapsed Time",
      "SignalExpression": null,
      "Queries": [
        {
          "Id": "chartquery-39",
          "Measurements": [
            {
              "Value": "min(Elapsed)",
              "Label": "MinElapsed"
            },
            {
              "Value": "max(Elapsed)",
              "Label": "MaxElapsed"
            },
            {
              "Value": "percentile(Elapsed, 50)",
              "Label": "MedianElapsed"
            },
            {
              "Value": "percentile(Elapsed, 90)",
              "Label": "P90Elapsed"
            },
            {
              "Value": "mean(Elapsed)",
              "Label": "MeanElapsed"
            }
          ],
          "Where": "RequestMethod = 'GET' and RequestPath like '/fllws%'",
          "SignalExpression": null,
          "GroupBy": [],
          "DisplayStyle": {
            "Type": "Bar",
            "LineFillToZeroY": false,
            "LineShowMarkers": true,
            "BarOverlaySum": false,
            "SuppressLegend": false,
            "Palette": "Default"
          },
          "Having": null,
          "OrderBy": [],
          "Limit": 10000
        }
      ],
      "DisplayStyle": {
        "WidthColumns": 6,
        "HeightRows": 1
      }
    },
    {
      "Id": "chart-40",
      "Title": "Get Public Timeline Count",
      "SignalExpression": null,
      "Queries": [
        {
          "Id": "chartquery-41",
          "Measurements": [
            {
              "Value": "count(*)",
              "Label": "count"
            }
          ],
          "Where": "RequestMethod = 'GET' and RequestPath = '/msgs'",
          "SignalExpression": null,
          "GroupBy": [],
          "DisplayStyle": {
            "Type": "Line",
            "LineFillToZeroY": false,
            "LineShowMarkers": true,
            "BarOverlaySum": false,
            "SuppressLegend": false,
            "Palette": "Blues"
          },
          "Having": null,
          "OrderBy": [],
          "Limit": 10000
        }
      ],
      "DisplayStyle": {
        "WidthColumns": 6,
        "HeightRows": 1
      }
    },
    {
      "Id": "chart-42",
      "Title": "Get Public Timeline Elapsed Time",
      "SignalExpression": null,
      "Queries": [
        {
          "Id": "chartquery-43",
          "Measurements": [
            {
              "Value": "min(Elapsed)",
              "Label": "MinElapsed"
            },
            {
              "Value": "max(Elapsed)",
              "Label": "MaxElapsed"
            },
            {
              "Value": "percentile(Elapsed, 50)",
              "Label": "MedianElapsed"
            },
            {
              "Value": "percentile(Elapsed, 90)",
              "Label": "P90Elapsed"
            },
            {
              "Value": "mean(Elapsed)",
              "Label": "MeanElapsed"
            }
          ],
          "Where": "RequestMethod = 'GET' and RequestPath = '/msgs'",
          "SignalExpression": null,
          "GroupBy": [],
          "DisplayStyle": {
            "Type": "Bar",
            "LineFillToZeroY": false,
            "LineShowMarkers": true,
            "BarOverlaySum": false,
            "SuppressLegend": false,
            "Palette": "Default"
          },
          "Having": null,
          "OrderBy": [],
          "Limit": 10000
        }
      ],
      "DisplayStyle": {
        "WidthColumns": 6,
        "HeightRows": 1
      }
    },
    {
      "Id": "chart-44",
      "Title": "Post Register Count",
      "SignalExpression": null,
      "Queries": [
        {
          "Id": "chartquery-45",
          "Measurements": [
            {
              "Value": "count(*)",
              "Label": "count"
            }
          ],
          "Where": "RequestMethod = 'POST' and RequestPath = '/register'",
          "SignalExpression": null,
          "GroupBy": [],
          "DisplayStyle": {
            "Type": "Line",
            "LineFillToZeroY": false,
            "LineShowMarkers": true,
            "BarOverlaySum": false,
            "SuppressLegend": false,
            "Palette": "OrangePurple"
          },
          "Having": null,
          "OrderBy": [],
          "Limit": 10000
        }
      ],
      "DisplayStyle": {
        "WidthColumns": 6,
        "HeightRows": 1
      }
    },
    {
      "Id": "chart-46",
      "Title": "Post Register Elapsed Time",
      "SignalExpression": null,
      "Queries": [
        {
          "Id": "chartquery-47",
          "Measurements": [
            {
              "Value": "min(Elapsed)",
              "Label": "MinElapsed"
            },
            {
              "Value": "max(Elapsed)",
              "Label": "MaxElapsed"
            },
            {
              "Value": "percentile(Elapsed, 50)",
              "Label": "MedianElapsed"
            },
            {
              "Value": "percentile(Elapsed, 90)",
              "Label": "P90Elapsed"
            },
            {
              "Value": "mean(Elapsed)",
              "Label": "MeanElapsed"
            }
          ],
          "Where": "RequestMethod = 'POST' and RequestPath = '/register'",
          "SignalExpression": null,
          "GroupBy": [],
          "DisplayStyle": {
            "Type": "Bar",
            "LineFillToZeroY": false,
            "LineShowMarkers": true,
            "BarOverlaySum": false,
            "SuppressLegend": false,
            "Palette": "Default"
          },
          "Having": null,
          "OrderBy": [],
          "Limit": 10000
        }
      ],
      "DisplayStyle": {
        "WidthColumns": 6,
        "HeightRows": 1
      }
    },
    {
      "Id": "chart-48",
      "Title": "Get Latest Count",
      "SignalExpression": null,
      "Queries": [
        {
          "Id": "chartquery-49",
          "Measurements": [
            {
              "Value": "count(*)",
              "Label": "count"
            }
          ],
          "Where": "RequestPath = '/latest'",
          "SignalExpression": null,
          "GroupBy": [],
          "DisplayStyle": {
            "Type": "Line",
            "LineFillToZeroY": false,
            "LineShowMarkers": true,
            "BarOverlaySum": false,
            "SuppressLegend": false,
            "Palette": "Reds"
          },
          "Having": null,
          "OrderBy": [],
          "Limit": 10000
        }
      ],
      "DisplayStyle": {
        "WidthColumns": 6,
        "HeightRows": 1
      }
    },
    {
      "Id": "chart-50",
      "Title": "Get Latest Elapsed Time",
      "SignalExpression": null,
      "Queries": [
        {
          "Id": "chartquery-51",
          "Measurements": [
            {
              "Value": "min(Elapsed)",
              "Label": "MinElapsed"
            },
            {
              "Value": "max(Elapsed)",
              "Label": "MaxElapsed"
            },
            {
              "Value": "percentile(Elapsed, 50)",
              "Label": "MedianElapsed"
            },
            {
              "Value": "percentile(Elapsed, 90)",
              "Label": "P90Elapsed"
            },
            {
              "Value": "mean(Elapsed)",
              "Label": "MeanElapsed"
            }
          ],
          "Where": "RequestPath = '/latest'",
          "SignalExpression": null,
          "GroupBy": [],
          "DisplayStyle": {
            "Type": "Bar",
            "LineFillToZeroY": false,
            "LineShowMarkers": true,
            "BarOverlaySum": false,
            "SuppressLegend": false,
            "Palette": "Default"
          },
          "Having": null,
          "OrderBy": [],
          "Limit": 10000
        }
      ],
      "DisplayStyle": {
        "WidthColumns": 6,
        "HeightRows": 1
      }
    }
  ],
  "Id": "dashboard-21",
  "Links": {
    "Self": "api/dashboards/dashboard-21?version=15",
    "Group": "api/dashboards/resources"
  }
}
```

## Business dashboard
```json
{
  "OwnerId": "user-admin",
  "Title": "Business",
  "IsProtected": false,
  "SignalExpression": null,
  "Charts": [
    {
      "Id": "chart-52",
      "Title": "Count Messages Per User (Find Top Users)",
      "SignalExpression": null,
      "Queries": [
        {
          "Id": "chartquery-53",
          "Measurements": [
            {
              "Value": "UserName, count(*)",
              "Label": "MessageCount"
            }
          ],
          "Where": "RequestMethod = 'POST' and RequestPath like '/msgs%'",
          "SignalExpression": null,
          "GroupBy": [
            "UserName"
          ],
          "DisplayStyle": {
            "Type": "Bar",
            "LineFillToZeroY": false,
            "LineShowMarkers": true,
            "BarOverlaySum": false,
            "SuppressLegend": false,
            "Palette": "Default"
          },
          "Having": null,
          "OrderBy": [
            "MessageCount desc"
          ],
          "Limit": 10000
        }
      ],
      "DisplayStyle": {
        "WidthColumns": 6,
        "HeightRows": 1
      }
    },
    {
      "Id": "chart-55",
      "Title": "Count Total Posts Over Time",
      "SignalExpression": null,
      "Queries": [
        {
          "Id": "chartquery-56",
          "Measurements": [
            {
              "Value": "count(*)",
              "Label": "TotalPosts"
            }
          ],
          "Where": "RequestMethod = 'POST' and RequestPath like '/msgs%'",
          "SignalExpression": null,
          "GroupBy": [],
          "DisplayStyle": {
            "Type": "Line",
            "LineFillToZeroY": true,
            "LineShowMarkers": true,
            "BarOverlaySum": false,
            "SuppressLegend": false,
            "Palette": "Default"
          },
          "Having": null,
          "OrderBy": [],
          "Limit": 10000
        }
      ],
      "DisplayStyle": {
        "WidthColumns": 6,
        "HeightRows": 1
      }
    },
    {
      "Id": "chart-57",
      "Title": "Post request duration trends 90-9-1",
      "SignalExpression": null,
      "Queries": [
        {
          "Id": "chartquery-58",
          "Measurements": [
            {
              "Value": "percentile(Elapsed, 99)",
              "Label": "SuperActiveUsersElapsed"
            },
            {
              "Value": "percentile(Elapsed, 90)",
              "Label": "ActiveUsersElapsed"
            },
            {
              "Value": "percentile(Elapsed, 1)",
              "Label": "Lurkers"
            },
            {
              "Value": "count(*)",
              "Label": "TotalPosts"
            }
          ],
          "Where": "RequestMethod = 'POST' and RequestPath like '/msgs%'",
          "SignalExpression": null,
          "GroupBy": [],
          "DisplayStyle": {
            "Type": "Line",
            "LineFillToZeroY": false,
            "LineShowMarkers": true,
            "BarOverlaySum": false,
            "SuppressLegend": false,
            "Palette": "Blues"
          },
          "Having": null,
          "OrderBy": [],
          "Limit": 10000
        }
      ],
      "DisplayStyle": {
        "WidthColumns": 6,
        "HeightRows": 1
      }
    },
    {
      "Id": "chart-59",
      "Title": "Count Total Follow Over Time",
      "SignalExpression": null,
      "Queries": [
        {
          "Id": "chartquery-60",
          "Measurements": [
            {
              "Value": "count(*)",
              "Label": "count"
            }
          ],
          "Where": "RequestMethod = 'POST' and RequestPath like '/fllws%'",
          "SignalExpression": null,
          "GroupBy": [],
          "DisplayStyle": {
            "Type": "Line",
            "LineFillToZeroY": true,
            "LineShowMarkers": true,
            "BarOverlaySum": false,
            "SuppressLegend": false,
            "Palette": "Default"
          },
          "Having": null,
          "OrderBy": [],
          "Limit": null
        }
      ],
      "DisplayStyle": {
        "WidthColumns": 6,
        "HeightRows": 1
      }
    },
    {
      "Id": "chart-61",
      "Title": "How often users fetch messages",
      "SignalExpression": null,
      "Queries": [
        {
          "Id": "chartquery-62",
          "Measurements": [
            {
              "Value": "UserName, count(*)",
              "Label": "GetRequests"
            }
          ],
          "Where": "RequestMethod = 'GET' and RequestPath like '/msgs%'",
          "SignalExpression": null,
          "GroupBy": [
            "UserName"
          ],
          "DisplayStyle": {
            "Type": "Line",
            "LineFillToZeroY": false,
            "LineShowMarkers": true,
            "BarOverlaySum": false,
            "SuppressLegend": false,
            "Palette": "Default"
          },
          "Having": null,
          "OrderBy": [
            "GetRequests desc"
          ],
          "Limit": 10000
        }
      ],
      "DisplayStyle": {
        "WidthColumns": 6,
        "HeightRows": 1
      }
    },
    {
      "Id": "chart-63",
      "Title": "How often users post messages",
      "SignalExpression": null,
      "Queries": [
        {
          "Id": "chartquery-64",
          "Measurements": [
            {
              "Value": "UserName, count(*)",
              "Label": "PostRequests"
            }
          ],
          "Where": "RequestMethod = 'POST' and RequestPath like '/msgs%'",
          "SignalExpression": null,
          "GroupBy": [
            "UserName"
          ],
          "DisplayStyle": {
            "Type": "Line",
            "LineFillToZeroY": false,
            "LineShowMarkers": true,
            "BarOverlaySum": false,
            "SuppressLegend": false,
            "Palette": "Default"
          },
          "Having": null,
          "OrderBy": [
            "PostRequests desc"
          ],
          "Limit": 10000
        }
      ],
      "DisplayStyle": {
        "WidthColumns": 6,
        "HeightRows": 1
      }
    }
  ],
  "Id": "dashboard-54",
  "Links": {
    "Self": "api/dashboards/dashboard-54?version=7",
    "Group": "api/dashboards/resources"
  }
}
```
