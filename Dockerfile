FROM docker/compose:latest

RUN apt install git

RUN apt install nodejs

RUN npm install -g smee-client

CMD [ "/supervisor/bootstrap.sh" ]