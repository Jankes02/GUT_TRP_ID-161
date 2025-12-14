All tests done in setup:

- Max concurrent users: 10000
- Users ramp up: 100 users per second
- Time: 5 minutes
- Waiting period: 1-3 seconds (it's how frequent user do requests, that's why requests concurrency (RPS) changes over time. )

most typical error with python:

> RemoteDisconnected('Remote end closed connection without response')
