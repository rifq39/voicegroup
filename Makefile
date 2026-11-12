.PHONY: rest-api websocket rest-api-down websocket-down

rest-api:
	$(MAKE) -C rest-api up

websocket:
	$(MAKE) -C websocket up

rest-api-down:
	$(MAKE) -C rest-api down

websocket-down:
	$(MAKE) -C websocket down
