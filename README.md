TODO: 
- Use containerized database and add to docker-compose
- ~~Containerize alarm system scripts and add to docker-compose~~
- ~~use containerized mosquitto and add to docker-compose~~
- ~~create script to easily transfer all secrets to home assistant host~~
- cleanup multi script loops in favor of new repeat mode
- ~~Setup stub secrets for futur travis integration~~
- ~~Setup travis to validate configs in PR~~
- Automatically update config for hass and restart when build pass on master
    - How? Can use github webhooks with the smee.io service to receive events without opening incoming port.
    - Then what? Create a docker image to run the smee.io client to receive events.
    - But we need to transfer the command to the host so it can run the deploy script
      - How? 
        - ssh into host? security concerns, but allow to directly run scripts on host. Or use Home Assistant's travis sensor, but still have similar issues..
        - named pipe or socket? we would need an always running service on the host that can receive the commands.. but would be nice to run everything from docker :(
        - any other solution? YES!
    - Better solution: by creating a docker image and sharing the docker daemon socket to it, it can then use docker-compose to spawn the other containers on the host docker daemon :D.
- Automatically update config for hass and restart when build pass on specific branches/commit/PR?
- ~~Read .HA_VERSION to pull correct image in travis~~
- Allow to build with latest Home assistant version to check config before upgrade Home Assistant
    - Would be nice if HAUpdate.sh:
      1) Stop and error if not on master or pending changes
      2) Pull latest master
      3) Starts a build in travis with latest Home assistant image
      4) Wait and verify for build success
      5) redeploy docker-compose
      6) Wait for complete deployment
      7) commit .HA_VERSION 
      8) push to master (On build success, should not redeploy to Home assistant host.. label? keyword in commit?)
