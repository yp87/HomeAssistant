FROM python:3.8-slim-buster

ENV LANG C.UTF-8

COPY run.sh ip150.py ip150_mqtt.py requirements.txt /
COPY secret_options.json /data/

RUN pip3 install -r requirements.txt

RUN chmod a+x /run.sh

CMD [ "/run.sh" ]