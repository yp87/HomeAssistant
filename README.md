TODO: 
- Use containerized database and add to docker-compose
- ~~Containerize alarm system scripts and add to docker-compose~~
- ~~use containerized mosquitto and add to docker-compose~~
- ~~create script to easily transfer all secrets to home assistant host~~
- HomeAssistant: cleanup multi script loops in favor of new repeat mode
- HomeAssistant: change custom_components huesensor to use git submodule that points to the original repo.
- ~~Setup stub secrets for futur travis integration~~
- ~~Setup travis to validate configs in PR~~
- Automatically update config for hass and restart when build pass on master
    - ~~Configure github webhook.~~
    - ~~Configure smee.io as proxy service for github webhook.~~
    - ~~Create webhook proxy container running smee.io client~~
    - ~~Create empty supervisor aspnetcore container to receive github events from webhook~~
    - ~~Authenticate web hook event in supervisor~~
    - Handle github event on build success on master to git pull + deploy homeassistant.
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
- ~~Check what can be done with Travis to only run home assistant config check when something actually changed in home assistant~~
- Travis build should build all the containers and validate the outcome (only if something changed that impact each container)
