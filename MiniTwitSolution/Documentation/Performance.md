# Performance

## 12.02.2025

### Caching using HybridCache
Guide used: https://www.youtube.com/watch?v=MQ96krIOjs4


HybridCache unifies in-memory and distributed caching in one API, delivering both low latency and scalable reliability in a simpler, more efficient solution than older, fragmented approaches.

## 18.02.2025

### Caching

The endpoints are using hybrid cache now with tags so that get calls to "/msgs?100" and "/msgs?200"
will both be updated after a post.


